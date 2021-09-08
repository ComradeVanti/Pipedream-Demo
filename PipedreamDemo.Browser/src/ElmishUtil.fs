[<AutoOpen>]
module PipedreamDemo.Browser.ElmishUtil

let viewAllBy viewFunc dispatch items =
    items |> List.map (fun item -> viewFunc item dispatch)
