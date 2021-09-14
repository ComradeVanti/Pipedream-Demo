[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.Editor

open Elmish
open Feliz
open PipedreamDemo
open PipedreamDemo.GraphManagement
open PipedreamDemo.LayoutManagement
open PipedreamDemo.GraphExecution

type State =
    {
        Graph: NodeGraph
        Layout: GraphLayout
        Inputs: InputValue list
        ClickedNodeIndex: NodeIndex option
    }

[<RequireQualifiedAccess>]
type Msg =
    | InputChanged of InputIndex * float
    | NodeClicked of NodeIndex
    | MouseUp
    | MouseDragged of Vector

let initialState =
    {
        Graph = fromNodes [ Input; Input; Output ] |> connect (0, 0) (2, 0)
        Layout = Positions [ XY(100., 100.); XY(100., 200.); XY(300., 150.) ]
        Inputs = [ 0.; 0. ]
        ClickedNodeIndex = None
    }

let setInputs inputs state = { state with Inputs = inputs }

let mapInputs mapper state = state |> setInputs (state.Inputs |> mapper)

let clickNode nodeIndex state = { state with ClickedNodeIndex = Some nodeIndex }

let unclick state = { state with ClickedNodeIndex = None }

let moveNodeWithIndexTo index newPos state =
    { state with
        Layout = state.Layout |> moveNode index newPos
    }

let moveClickedNodeTo newPos state =
    match state.ClickedNodeIndex with
    | Some index -> state |> moveNodeWithIndexTo index newPos
    | None -> state

let init _ = initialState, Cmd.none

let update msg state =
    match msg with
    | Msg.InputChanged (index, value) ->
        (state |> mapInputs (replaceAtIndex value index)), Cmd.none
    | Msg.NodeClicked index -> state |> clickNode index, Cmd.none
    | Msg.MouseUp -> state |> unclick, Cmd.none
    | Msg.MouseDragged newPos -> state |> moveClickedNodeTo newPos, Cmd.none

let viewNode node index (XY (x, y)) (value: NodeValue) dispatch =

    let viewInputContent () =
        Html.input [ prop.classes [ "node-content"; "input" ]
                     prop.value value
                     prop.type' "number"
                     prop.onChange
                         (fun v -> dispatch (Msg.InputChanged(index, v)))
                     prop.onMouseDown (fun e -> e.stopPropagation ()) ]

    let viewOutputContent () =
        Html.div [ prop.classes [ "node-content"; "output" ]
                   prop.text (string value) ]

    let viewInputSlot () = Html.div [ prop.className "node-slot" ]

    let viewOutputSlot () = Html.div [ prop.className "node-slot" ]

    let inputSlots =
        Html.div [ prop.className "slots-container"
                   prop.children (
                       match node with
                       | Input -> []
                       | Output -> [ viewInputSlot () ]
                   ) ]

    let nodeContent =
        match node with
        | Input -> viewInputContent ()
        | Output -> viewOutputContent ()

    let outputSlots =
        Html.div [ prop.className "slots-container"
                   prop.children (
                       match node with
                       | Input -> [ viewOutputSlot () ]
                       | Output -> []
                   ) ]

    let nodeBody =
        Html.div [ prop.className "node-body"
                   prop.onMouseDown
                       (fun e ->
                           dispatch (Msg.NodeClicked index)
                           e.stopPropagation ())
                   prop.children [ Html.text (string node); nodeContent ] ]

    Html.div [ prop.className "node"
               prop.style [ style.transform (transform.translate (x, y)) ]
               prop.children [ inputSlots; nodeBody; outputSlots ] ]

let view state dispatch =

    let graph = state.Graph
    let layout = state.Layout
    let values = graph |> run state.Inputs

    let viewNodeAtIndex value index =
        let node = graph |> nodeAt index
        let position = layout |> positionAt index
        viewNode node index position value dispatch

    let nodes =
        List.init (state.Graph |> nodeCount) id
        |> List.map (fun index -> viewNodeAtIndex values.[index] index)

    Html.div [ prop.id "editor"
               prop.children nodes
               prop.onMouseUp (fun _ -> dispatch Msg.MouseUp)
               prop.onMouseMove
                   (fun e ->
                       if state.ClickedNodeIndex |> Option.isSome then
                           dispatch (Msg.MouseDragged(XY(e.clientX, e.clientY)))
                           e.preventDefault ()) ]
