module PipedreamDemo.BuiltinPipes

let plus =
    {
        InputCount = 2
        OutputCount = 1
        Name = "Plus"
        Func = (fun inputs -> [ inputs.[0] + inputs.[1] ])
    }

let minus =
    {
        InputCount = 2
        OutputCount = 1
        Name = "Minus"
        Func = (fun inputs -> [ inputs.[0] - inputs.[1] ])
    }

let times =
    {
        InputCount = 2
        OutputCount = 1
        Name = "Times"
        Func = (fun inputs -> [ inputs.[0] * inputs.[1] ])
    }

let over =
    {
        InputCount = 2
        OutputCount = 1
        Name = "Over"
        Func = (fun inputs -> [ inputs.[0] / inputs.[1] ])
    }
