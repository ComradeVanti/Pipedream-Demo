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

let private viewInputs inputs dispatch = Html.div [ prop.id "inputs" ]

let view state dispatch =
    Html.div [ prop.id "editor"
               prop.children [ viewInputs state.Inputs dispatch ] ]
