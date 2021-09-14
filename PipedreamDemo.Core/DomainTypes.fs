[<AutoOpen>]
module PipedreamDemo.DomainTypes

type NodeValue = float

type InputValue = NodeValue

type Node =
    | Input
    | Output

type NodeGraph = Nodes of Node list

type Vector = XY of float * float

type NodePosition = Vector

type GraphLayout = Positions of NodePosition list

type GraphValues = NodeValue list
