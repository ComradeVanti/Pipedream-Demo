[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.ViewEditor

open Browser.Types
open Feliz
open PipedreamDemo
open PipedreamDemo.GraphManagement
open PipedreamDemo.LayoutManagement
open PipedreamDemo.GraphExecution

let viewNode
    node
    index
    (XY (x, y))
    (values: GraphValues)
    (inputs: InputValue list)
    dispatch
    =

    let onBodyMouseDown (e: MouseEvent) =
        dispatch (Editor.Msg.NodeClicked index)
        e.stopPropagation ()

    let onOutputMouseDown address (e: MouseEvent) =
        dispatch (Editor.Msg.OutputClicked address)
        e.stopPropagation ()

    let onInputMouseUp address _ = dispatch (Editor.Msg.MouseUpOnInput address)

    let onInputClicked address _ = dispatch (Editor.Msg.InputClicked address)

    let viewInputContent () =
        Html.input [ prop.classes [ "node-content"; "input" ]
                     prop.value (inputs |> List.item index)
                     prop.type' "number"
                     prop.onChange
                         (fun v -> dispatch (Editor.Msg.InputChanged(index, v)))
                     prop.onMouseDown (fun e -> e.stopPropagation ()) ]

    let viewOutputContent () =
        Html.div [ prop.classes [ "node-content"; "output" ] ]

    let viewPipeCallContent () =
        Html.div [ prop.classes [ "node-content"; "pipe-call" ] ]

    let viewInputSlot slotIndex =
        let address = (index, slotIndex)

        Html.div [ prop.className "node-slot"
                   prop.id $"{index}-input-{slotIndex}"
                   prop.onMouseUp (onInputMouseUp address)
                   prop.onClick (onInputClicked address) ]

    let viewOutputSlot slotIndex =
        let address = (index, slotIndex)

        let value =
            values
            |> Map.tryFind address
            |> Option.map string
            |> Option.defaultValue "?"

        Html.div [ prop.className "node-slot"
                   prop.id $"{index}-output-{slotIndex}"
                   prop.text value
                   prop.onMouseDown (onOutputMouseDown address) ]

    let inputCount node =
        match node with
        | Input -> 0
        | Output -> 1
        | PipeCall pipe -> pipe.InputCount

    let outputCount node =
        match node with
        | Input -> 1
        | Output -> 0
        | PipeCall pipe -> pipe.OutputCount

    let viewSlots count (viewFunction: int -> ReactElement) =
        Html.div [ prop.className "slots-container"
                   prop.children (List.init count id |> List.map viewFunction) ]

    let inputSlots = viewSlots (node |> inputCount) viewInputSlot

    let nodeName =
        match node with
        | Input -> "Input"
        | Output -> "Output"
        | PipeCall pipe -> pipe.Name

    let nodeContent =
        match node with
        | Input -> viewInputContent ()
        | Output -> viewOutputContent ()
        | PipeCall _ -> viewPipeCallContent ()

    let outputSlots = viewSlots (node |> outputCount) viewOutputSlot

    let nodeBody =
        Html.div [ prop.className "node-body"
                   prop.onMouseDown onBodyMouseDown
                   prop.children [ Html.text nodeName; nodeContent ] ]

    Html.div [ prop.className "node"
               prop.style [ style.transform (transform.translate (x, y)) ]
               prop.children [ inputSlots; nodeBody; outputSlots ] ]

let asIdentifier address = $"{fst address}-{snd address}"

let generateLinkId (Endpoints (input, output)) =
    $"{input |> asIdentifier} to {output |> asIdentifier}"

let viewLinkWithId id = Svg.line [ svg.id id; svg.className "link" ]

let viewLink link = link |> generateLinkId |> viewLinkWithId

let viewLinkToMouse outputAddress =
    $"{outputAddress |> asIdentifier} to mouse"
    |> viewLinkWithId

let viewPipeElements graph values inputs layout dispatch =

    let calcRelativeMousePosition (e: MouseEvent) =
        let target = e.currentTarget :?> HTMLElement
        let rect = target.getBoundingClientRect ()
        XY(e.clientX - rect.left, e.clientY - rect.top)

    let onMouseMoved (e: MouseEvent) =
        if e.buttons = 1. then
            e
            |> calcRelativeMousePosition
            |> Editor.Msg.MouseDragged
            |> dispatch

            e.preventDefault ()

    let viewNodeAtIndex index =
        let node = graph |> nodeAt index
        let position = layout |> positionAt index
        viewNode node index position values inputs dispatch

    let nodeIds = List.init (graph |> nodeCount) id
    let nodes = nodeIds |> List.map viewNodeAtIndex

    Html.div [ prop.id "pipe-elements"
               prop.children nodes
               prop.onMouseMove onMouseMoved ]

let viewButtonBar dispatch =

    let viewButton (name: string) pipe =
        Html.button [ prop.className "pipe-button"
                      prop.text name
                      prop.onClick (fun _ -> dispatch (Editor.Msg.AddPipe pipe)) ]

    let plusButton = viewButton "Plus" BuiltinPipes.plus
    let minusButton = viewButton "Minus" BuiltinPipes.minus
    let timesButton = viewButton "Times" BuiltinPipes.times
    let overButton = viewButton "Over" BuiltinPipes.over

    Html.div [ prop.id "button-bar"
               prop.children [ Html.text "Add new pipes"
                               plusButton
                               minusButton
                               timesButton
                               overButton ] ]

let view (state: Editor.State) dispatch =

    let graph = state.Graph
    let layout = state.Layout
    let values = graph |> run state.Inputs

    let mouseLink = state.ClickedOutputSlot |> Option.map viewLinkToMouse

    let buttonBar = viewButtonBar dispatch

    let pipeElements =
        viewPipeElements graph values state.Inputs layout dispatch

    let links =
        graph.Links
        |> List.map viewLink
        |> appendIfPresent mouseLink
        |> Svg.svg

    Html.div [ prop.id "editor"
               prop.children [ buttonBar; pipeElements; links ]
               prop.onMouseUp (fun _ -> dispatch Editor.Msg.MouseUp) ]
