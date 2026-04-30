import {createRoot} from "react-dom/client";
import TagSearch from "./home/tagSearch.jsx";

const tagSearch = document.getElementById('tag-search-root');
if (tagSearch) {
    const root = createRoot(tagSearch);
    root.render(<TagSearch/>);
}