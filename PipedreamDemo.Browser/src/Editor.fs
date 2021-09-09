[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.Editor

open Elmish
open Feliz
open PipedreamDemo
open PipedreamDemo.GraphManagement
open PipedreamDemo.LayoutManagement

type InputIndex = int

type State =
    {
        Graph: NodeGraph
        Layout: GraphLayout
        Inputs: InputValue list
    }

[<RequireQualifiedAccess>]
type Msg = InputChanged of InputIndex * float

let initialState =
    {
        Graph = fromNodes [ Input; Input ]
        Layout = Positions [ (XY(100., 100.)); (XY(100., 200.)) ]
        Inputs = [ 0.; 0. ]
    }

let setInputs inputs state = { state with Inputs = inputs }

let mapInputs mapper state = state |> setInputs (state.Inputs |> mapper)

let init _ = initialState, Cmd.none

let update msg state =
    match msg with
    | Msg.InputChanged (index, value) ->
        (state |> mapInputs (replaceAtIndex value index)), Cmd.none

let viewInput index (XY (x, y)) (value: NodeValue) dispatch =
    Html.input [ prop.classes [ "node"; "input" ]
                 prop.value value
                 prop.type' "number"
                 prop.onChange (fun v -> dispatch (Msg.InputChanged(index, v)))
                 prop.style [ style.transform (transform.translate (x, y)) ] ]

let viewNode node index position value dispatch =
    match node with
    | Input -> viewInput index position value dispatch

let viewNodeAtIndex state dispatch index =
    viewNode
        (state.Graph |> nodeAt index)
        index
        (state.Layout |> positionAt index)
        state.Inputs.[index]
        dispatch

let viewNodes state dispatch =
    List.init (state.Graph |> nodeCount) id
    |> List.map (viewNodeAtIndex state dispatch)

let view state dispatch =
    Html.div [ prop.id "editor"
               prop.children (viewNodes state dispatch) ]
