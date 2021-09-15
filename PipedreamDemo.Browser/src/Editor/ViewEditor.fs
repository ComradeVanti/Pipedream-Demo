[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.ViewEditor

open Browser.Types
open Feliz
open PipedreamDemo
open PipedreamDemo.GraphManagement
open PipedreamDemo.LayoutManagement
open PipedreamDemo.GraphExecution

let viewNode node index (XY (x, y)) (value: NodeValue) dispatch =

    let viewInputContent () =
        Html.input [ prop.classes [ "node-content"; "input" ]
                     prop.value value
                     prop.type' "number"
                     prop.onChange
                         (fun v -> dispatch (Editor.Msg.InputChanged(index, v)))
                     prop.onMouseDown (fun e -> e.stopPropagation ()) ]

    let viewOutputContent () =
        Html.div [ prop.classes [ "node-content"; "output" ]
                   prop.text (string value) ]

    let viewInputSlot slotIndex =
        Html.div [ prop.className "node-slot"
                   prop.id $"{index}-input-{slotIndex}" ]

    let viewOutputSlot slotIndex =
        Html.div [ prop.className "node-slot"
                   prop.id $"{index}-output-{slotIndex}" ]

    let onBodyMouseDown (e: MouseEvent) =
        dispatch (Editor.Msg.NodeClicked index)
        e.stopPropagation ()

    let inputSlots =
        Html.div [ prop.className "slots-container"
                   prop.children (
                       match node with
                       | Input -> []
                       | Output -> [ viewInputSlot 0 ]
                   ) ]

    let nodeContent =
        match node with
        | Input -> viewInputContent ()
        | Output -> viewOutputContent ()

    let outputSlots =
        Html.div [ prop.className "slots-container"
                   prop.children (
                       match node with
                       | Input -> [ viewOutputSlot 0 ]
                       | Output -> []
                   ) ]

    let nodeBody =
        Html.div [ prop.className "node-body"
                   prop.onMouseDown onBodyMouseDown
                   prop.children [ Html.text (string node); nodeContent ] ]

    Html.div [ prop.className "node"
               prop.style [ style.transform (transform.translate (x, y)) ]
               prop.children [ inputSlots; nodeBody; outputSlots ] ]

let generateLinkId (Endpoints (input, output)) =
    $"{fst input}-{snd input} to {fst output}-{snd output}"

let viewLink link =
    Svg.line [ svg.id (link |> generateLinkId)
               svg.className "link" ]

let view (state: Editor.State) dispatch =

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

    let links = Svg.svg (graph.Links |> List.map viewLink)

    let onMouseMoved (e: MouseEvent) =
        if state.ClickedNodeIndex |> Option.isSome then
            dispatch (Editor.Msg.MouseDragged(XY(e.clientX, e.clientY)))
            e.preventDefault ()

    Html.div [ prop.id "editor"
               prop.children (nodes |> appendItem links)
               prop.onMouseUp (fun _ -> dispatch Editor.Msg.MouseUp)
               prop.onMouseMove onMouseMoved ]
