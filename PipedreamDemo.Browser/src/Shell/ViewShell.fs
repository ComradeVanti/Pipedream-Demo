[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.ViewShell

open Feliz

let view (state: Shell.State) dispatch =

    let viewEditor editorState =
        ViewEditor.view editorState (Shell.Msg.Editor >> dispatch)

    let pageContent =
        match state with
        | Shell.State.Editor editorState -> (viewEditor editorState)

    Html.div [ prop.id "shell"; prop.children [ pageContent ] ]
