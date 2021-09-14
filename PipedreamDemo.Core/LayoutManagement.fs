module PipedreamDemo.LayoutManagement

let positionAt index (Positions positions) = positions.[index]

let moveNode index newPosition (Positions positions) =
    positions |> replaceAtIndex newPosition index |> Positions
