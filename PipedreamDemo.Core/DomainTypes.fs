[<AutoOpen>]
module PipedreamDemo.DomainTypes

type NodeValue = float

type InputValue = NodeValue

type Node = | Input

type NodeGraph = Nodes of Node list

type NodePosition = XY of float * float

type GraphLayout = Positions of NodePosition list
