[<RequireQualifiedAccess>]
module PipedreamDemo.Browser.ViewShell

open Feliz

let view (state: Shell.State) dispatch =
    Html.div [ prop.id "shell"
               prop.children [ ViewEditor.view
                                   state.Editor
                                   (Shell.Msg.Editor >> dispatch) ] ]
