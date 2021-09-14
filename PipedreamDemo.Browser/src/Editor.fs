﻿[<RequireQualifiedAccess>]
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
        Graph = fromNodes [ Input; Input; Output ]
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

let viewInput index (value: NodeValue) dispatch =
    Html.input [ prop.className "input"
                 prop.value value
                 prop.type' "number"
                 prop.onChange (fun v -> dispatch (Msg.InputChanged(index, v))) ]

let viewOutput value =
    Html.div [ prop.className "output"
               prop.text (string value) ]

let viewNode node index (XY (x, y)) value dispatch =
    Html.div [ prop.className "node"
               prop.style [ style.transform (transform.translate (x, y)) ]
               prop.onMouseDown (fun _ -> dispatch (Msg.NodeClicked index))
               prop.children [ match node with
                               | Input -> viewInput index value dispatch
                               | Output -> viewOutput value ] ]

let viewNodeAtIndex state value index dispatch =
    viewNode
        (state.Graph |> nodeAt index)
        index
        (state.Layout |> positionAt index)
        value
        dispatch

let viewNodes state dispatch =
    let values = state.Graph |> run state.Inputs

    List.init (state.Graph |> nodeCount) id
    |> List.map
        (fun index -> viewNodeAtIndex state values.[index] index dispatch)

let view state dispatch =
    Html.div [ prop.id "editor"
               prop.children (viewNodes state dispatch)
               prop.onMouseUp (fun _ -> dispatch Msg.MouseUp)
               prop.onMouseMove
                   (fun e ->
                       if state.ClickedNodeIndex |> Option.isSome then
                           dispatch (Msg.MouseDragged(XY(e.clientX, e.clientY)))) ]
