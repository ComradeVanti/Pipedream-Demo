[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.Shell

open Elmish
open Feliz

[<RequireQualifiedAccess>]
type State = unit

[<RequireQualifiedAccess>]
type Msg = unit

let init _ : State * Cmd<Msg> = (), Cmd.none

let update msg state : State * Cmd<Msg> = state, Cmd.none

let view dispatch state = Html.div [ prop.id "shell" ]
