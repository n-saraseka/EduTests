function TestTag({name}) {
    return <a href={`/tag/${name}`} className="card-tag">{name}</a>;
}

export default TestTag;