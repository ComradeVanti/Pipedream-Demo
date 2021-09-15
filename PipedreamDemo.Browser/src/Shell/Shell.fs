[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.Shell

open Elmish

type State = Editor of Editor.State

[<RequireQualifiedAccess>]
type Msg = Editor of Editor.Msg

let private wrapEditor (state, cmd) = Editor state, cmd |> Cmd.map Msg.Editor

let init arg = Editor.init arg |> wrapEditor

let update msg state =
    match msg, state with
    | Msg.Editor editorMsg, Editor editorState ->
        editorState |> Editor.update editorMsg |> wrapEditor
