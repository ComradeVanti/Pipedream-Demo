module PipedreamDemo.GraphExecution

open PipedreamDemo
open PipedreamDemo.GraphManagement

type SlotValue = float

type NodeInput = SlotValue option list

type NodeOutput = SlotValue list

type NetworkNode =
    {
        Inputs: NodeInput
        Func: PipeFunc
        Outputs: NodeOutput option
    }

type Network = { Nodes: NetworkNode list; Links: Link list }

let private buildNetworkNode node =
    let inputs count = List.replicate count None

    match node with
    | Input -> { Inputs = inputs 1; Func = id; Outputs = None }
    | Output -> { Inputs = inputs 1; Func = id; Outputs = None }
    | PipeCall pipe ->
        {
            Inputs = inputs pipe.InputCount
            Func = pipe.Func
            Outputs = None
        }

let private buildNetwork (graph: NodeGraph) =
    {
        Nodes = graph.Nodes |> List.map buildNetworkNode
        Links = graph.Links
    }

let private setInput slotIndex value node =
    { node with
        Inputs = node.Inputs |> replaceAtIndex (Some value) slotIndex
    }

let private feedInputs inputs network =

    let feedInput nodeIndex value network : Network =
        { network with
            Nodes = network.Nodes |> mapAtIndex (setInput 0 value) nodeIndex
        }

    network
    |> feedInput 0 (inputs |> List.item 0)
    |> feedInput 1 (inputs |> List.item 1)

let private isDone network = network.Nodes.[2].Outputs |> Option.isSome

let nodeValues network =
    network.Nodes |> List.map (fun n -> (n.Inputs, n.Outputs))

let isSameAs n1 n2 = (n1 |> nodeValues) = (n2 |> nodeValues)

let private propagateUntilDone network : Network =

    let rec propagate network : Network =

        let tryGetValueForOutputSlot (nodeIndex, slotIndex) =
            network.Nodes
            |> List.tryItem nodeIndex
            |> Option.bind
                (fun n -> n.Outputs |> Option.bind (List.tryItem slotIndex))

        let getInputValueFor (Endpoints (address, _)) =
            tryGetValueForOutputSlot address

        let tryGetValueForInputSlot address =
            network.Links
            |> List.tryFind (hasOutput address)
            |> Option.bind getInputValueFor

        let pullInput nodeIndex slotIndex currVal =
            match currVal with
            | Some value -> Some value
            | None -> tryGetValueForInputSlot (nodeIndex, slotIndex)

        let pullInputs nodeIndex node =
            { node with
                Inputs = node.Inputs |> List.mapi (pullInput nodeIndex)
            }

        let hasAllInputs node = node.Inputs |> List.forall Option.isSome

        let runNode node =
            let inputs = node.Inputs |> List.map Option.get
            let outputs = inputs |> node.Func
            { node with Outputs = Some outputs }

        let runIfHasAllInputs node =
            if node |> hasAllInputs then runNode node else node

        let propagateNode nodeIndex node =
            node |> pullInputs nodeIndex |> runIfHasAllInputs

        let updated =
            { network with
                Nodes = network.Nodes |> List.mapi propagateNode
            }

        if updated |> isDone || updated |> isSameAs network then
            updated
        else
            updated |> propagate

    network |> propagate

let private readValues network : GraphValues =

    let readFromSlot nodeIndex index slot =
        let address = (nodeIndex, index)
        address, slot

    let readFromNode index node =
        node.Outputs
        |> Option.map (List.mapi (readFromSlot index))
        |> Option.defaultValue []

    network.Nodes
    |> List.mapi readFromNode
    |> List.concat
    |> Map.ofList

let run inputs graph =
    graph
    |> buildNetwork
    |> feedInputs inputs
    |> propagateUntilDone
    |> readValues
