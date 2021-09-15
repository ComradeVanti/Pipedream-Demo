[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.Editor

open Elmish
open PipedreamDemo
open PipedreamDemo.GraphManagement
open PipedreamDemo.LayoutManagement

type State =
    {
        Graph: NodeGraph
        Layout: GraphLayout
        Inputs: InputValue list
        ClickedNodeIndex: NodeIndex option
        ClickedOutputSlot: SlotAddress option
    }

[<RequireQualifiedAccess>]
type Msg =
    | InputChanged of InputIndex * float
    | NodeClicked of NodeIndex
    | OutputClicked of SlotAddress
    | MouseUp
    | MouseDragged of Vector

let initialState =
    {
        Graph = fromNodes [ Input; Input; Output ] |> connect (0, 0) (2, 0)
        Layout = Positions [ XY(100., 100.); XY(100., 200.); XY(300., 150.) ]
        Inputs = [ 0.; 0. ]
        ClickedNodeIndex = None
        ClickedOutputSlot = None
    }

let setInputs inputs state = { state with Inputs = inputs }

let mapInputs mapper state = state |> setInputs (state.Inputs |> mapper)

let clickNode nodeIndex state = { state with ClickedNodeIndex = Some nodeIndex }

let clickOutput address state = { state with ClickedOutputSlot = Some address }

let unclick state =
    { state with
        ClickedNodeIndex = None
        ClickedOutputSlot = None
    }

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
    | Msg.OutputClicked address -> state |> clickOutput address, Cmd.none
    | Msg.MouseUp -> state |> unclick, Cmd.none
    | Msg.MouseDragged newPos -> state |> moveClickedNodeTo newPos, Cmd.none
