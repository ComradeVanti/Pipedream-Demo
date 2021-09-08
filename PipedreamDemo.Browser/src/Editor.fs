[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.Editor

open Elmish
open Feliz
open PipedreamDemo

type State = { Inputs: Input list }

[<RequireQualifiedAccess>]
type Msg = unit

let init _ : State * Cmd<Msg> =
    { Inputs = [ InputValue 0.; InputValue 0. ] }, Cmd.none

let update msg state : State * Cmd<Msg> = state, Cmd.none

let private viewInput (InputValue value) dispatch =
    Html.div [ prop.className "input"; prop.text value ]

let private viewInputs inputs dispatch =
    Html.div [ prop.id "inputs"
               prop.children (inputs |> viewAllBy viewInput dispatch) ]

let view state dispatch =
    Html.div [ prop.id "editor"
               prop.children [ viewInputs state.Inputs dispatch ] ]
