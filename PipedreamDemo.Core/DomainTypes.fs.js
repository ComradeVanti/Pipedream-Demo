import { Union } from "../PipedreamDemo.Browser/src/.fable/fable-library.3.2.9/Types.js";
import { union_type, float64_type } from "../PipedreamDemo.Browser/src/.fable/fable-library.3.2.9/Reflection.js";

export class Input extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["InputValue"];
    }
}

export function Input$reflection() {
    return union_type("PipedreamDemo.DomainTypes.Input", [], Input, () => [[["Item", float64_type]]]);
}

