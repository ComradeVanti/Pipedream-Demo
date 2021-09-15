let createPos = (x, y) => {
    return {x: x, y: y}
}

let currentMousePos = createPos(0, 0)

function getCenterPosition(element) {
    let rect = element.getBoundingClientRect();
    return createPos(
        rect.left + (rect.width / 2),
        rect.top + (rect.height / 2))
}

let connectPositionsWithLine = (p1, p2, line) => {
    line.x1.baseVal.value = p1.x
    line.y1.baseVal.value = p1.y
    line.x2.baseVal.value = p2.x
    line.y2.baseVal.value = p2.y
}

let createOutputId = (address) =>
    `${address[0]}-output-${address[1]}`

let createInputId = (address) =>
    `${address[0]}-input-${address[1]}`

let getOutputElement = (address) =>
    document.getElementById(createOutputId(address))

let getInputElement = (address) =>
    document.getElementById(createInputId(address))

let getOutputPosition = (address) =>
    getCenterPosition(getOutputElement(address))

let getInputPosition = (address) =>
    getCenterPosition(getInputElement(address))

let connectAddressesWithLine = (a1, a2, line) =>
    connectPositionsWithLine(
        getOutputPosition(a1),
        getInputPosition(a2),
        line)

let connectAddressAndMouseWithLine = (address, line) => {
    let addressPosition = getOutputPosition(address)
    connectPositionsWithLine(addressPosition, currentMousePos, line)
}

let moveLine = line => {
    let slotIdentifiers = line.id.split(" to ")

    let outputAddress = slotIdentifiers[0].split("-")

    if (slotIdentifiers[1] === "mouse") {
        connectAddressAndMouseWithLine(outputAddress, line)
    } else {
        let inputAddress = slotIdentifiers[1].split("-")
        connectAddressesWithLine(outputAddress, inputAddress, line)
    }
}

let refreshLines = () => {
    for (let line of document.getElementsByTagName("line"))
        moveLine(line)
}

window.onload = refreshLines
window.onmousemove = e => {
    currentMousePos = createPos(e.clientX, e.clientY)
    refreshLines()
}