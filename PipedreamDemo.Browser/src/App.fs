module PipedreamDemo.Browser.App

open Elmish
open Elmish.React

Program.mkProgram Shell.init Shell.update ViewShell.view
|> Program.withConsoleTrace
|> Program.withReactBatched "root"
|> Program.run
