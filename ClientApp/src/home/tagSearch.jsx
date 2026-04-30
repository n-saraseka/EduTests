import {useState} from "react";

function TagSearch() {
    const [query, setQuery] = useState("");
    const [tags, setTags] = useState([]);
    
    const onQueryChange = async (event) => {
        const newQuery = event.target.value;
        if (newQuery !== query) {
            setQuery(newQuery);
            if (newQuery === "") {
                setTags([]);
            }
            else {
                await getTags(newQuery);
            }
        }
    }
    
    const getTags = async (query) => {
        const response = await searchTags(query);
        if (response.ok) {
            const newTags = await response.json();
            setTags(newTags);
        }
    }
    
    return (<>
        <input type="text" id="tag-search" name="tag-search" maxLength={32} value={query} onChange={onQueryChange} 
               placeholder="Поиск тегов..." autoFocus={true}/>
        {tags.length > 0 && <ul id="search-tags">
            {tags.map(tag => (<li key={tag.id}><a href={`/?tagName=${tag.name}`}>{tag.name}</a></li>))}
        </ul>}
    </>)
}

function searchTags(query) {
    return fetch(`/api/tags/search?query=${query}`);
}

export default TagSearch;