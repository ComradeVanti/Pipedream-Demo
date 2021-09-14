[<AutoOpen>]
module PipedreamDemo.ListUtil

let mapAtIndex mapper index list =
    list
    |> List.mapi (fun i item -> if i = index then mapper item else item)

let replaceAtIndex newItem index list =
    list |> mapAtIndex (fun _ -> newItem) index
    
let appendItem item list =
    List.append list [item]
