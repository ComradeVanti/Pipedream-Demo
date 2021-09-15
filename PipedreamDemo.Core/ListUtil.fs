[<AutoOpen>]
module PipedreamDemo.ListUtil

let mapAtIndex mapper index list =
    list
    |> List.mapi (fun i item -> if i = index then mapper item else item)

let replaceAtIndex newItem index list =
    list |> mapAtIndex (fun _ -> newItem) index

let appendItem item list = List.append list [ item ]

let appendIfPresent itemOption list =
    match itemOption with
    | Some item -> list |> appendItem item
    | None -> list
