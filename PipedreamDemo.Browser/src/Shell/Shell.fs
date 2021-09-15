[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.Shell

open Elmish

type State = { Editor: Editor.State }

[<RequireQualifiedAccess>]
type Msg = Editor of Editor.Msg

let private wrapEditor (state, cmd) =
    { Editor = state }, cmd |> Cmd.map Msg.Editor

let private applyEditor state (editorState, editorCmd) =
    { state with Editor = editorState }, editorCmd |> Cmd.map Msg.Editor

let init arg = Editor.init arg |> wrapEditor

let update msg state =
    match msg with
    | Msg.Editor editorMsg ->
        state.Editor |> Editor.update editorMsg |> applyEditor state
