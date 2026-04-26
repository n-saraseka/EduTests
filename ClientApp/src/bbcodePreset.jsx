import presetReact from "@bbob/preset-react";
import BBCode from "@bbob/react";

const preset = presetReact.extend((tags, options) => ({
    ...tags,
    center: (node) => ({
        tag: "span",
        content: node.content,
        attrs: {style: {textAlign: "center"}}
    })
}))();

const allowedTags = ['b', 'i', 'img', 'center', 'u', 'color', 's', 'quote', 'url', 'ul', 'ol', 'li', 'table', 'tr', 'td', 'th'];
const plugins = [preset];

function BbcodePreset({text}) {
    return(<BBCode plugins={plugins} options={{onlyAllowTags: allowedTags}}>{text}</BBCode>);
}

export default BbcodePreset;