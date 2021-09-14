function getCenterPosition(element) {
    let rect = element.getBoundingClientRect();
    let x = rect.left + (rect.width / 2)
    let y = rect.top + (rect.height / 2)
    return {x: x, y: y}
}

let connectPositionsWithLine = (p1, p2, line) => {
    line.x1.baseVal.value = p1.x
    line.y1.baseVal.value = p1.y
    line.x2.baseVal.value = p2.x
    line.y2.baseVal.value = p2.y
}

let connectElementsWithLine = (e1, e2, line) => {
    connectPositionsWithLine(
        getCenterPosition(e1),
        getCenterPosition(e2),
        line)
}

let moveLine = line => {
    let slotIdentifiers = line.id.split(" to ")
    let inputSlotIdentifier = slotIdentifiers[0].split("-")
    let outputSlotIdentifier = slotIdentifiers[1].split("-")

    let inputSlotId = `${inputSlotIdentifier[0]}-output-${inputSlotIdentifier[1]}`
    let outputSlotId = `${outputSlotIdentifier[0]}-input-${outputSlotIdentifier[1]}`

    connectElementsWithLine(
        document.getElementById(inputSlotId),
        document.getElementById(outputSlotId),
        line)
}

let refreshLines = () => {
    for (let line of document.getElementsByTagName("line"))
        moveLine(line)
}

window.onload = refreshLines
window.onmousemove = e => {
    if (e.buttons === 1) refreshLines()
}