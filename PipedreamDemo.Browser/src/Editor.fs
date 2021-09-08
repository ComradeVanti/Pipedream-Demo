[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.Editor

open Elmish
open Feliz
open PipedreamDemo

type InputIndex = int

type State = { Inputs: Input list }

[<RequireQualifiedAccess>]
type Msg = InputChanged of InputIndex * float

let init _ : State * Cmd<Msg> =
    { Inputs = [ InputValue 0.; InputValue 0. ] }, Cmd.none

let update msg state =
    match msg with
    | Msg.InputChanged (index, value) ->
        { state with
            Inputs = state.Inputs |> replaceAtIndex (InputValue value) index
        },
        Cmd.none

let private viewInput (InputValue value) index dispatch =
    Html.input [ prop.className "input"
                 prop.value value
                 prop.type' "number"
                 prop.onChange (fun v -> dispatch (Msg.InputChanged(index, v))) ]

let private viewInputs inputs dispatch =
    Html.div [ prop.id "inputs"
               prop.children (
                   List.append
                       [ Html.h2 [ prop.text "Inputs" ] ]
                       (inputs
                        |> List.mapi (fun i input -> viewInput input i dispatch))
               ) ]

let view state dispatch =
    Html.div [ prop.id "editor"
               prop.children [ viewInputs state.Inputs dispatch ] ]
