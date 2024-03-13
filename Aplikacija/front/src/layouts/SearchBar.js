import useFetch from "../useFetch"
import { useNavigate  } from "react-router-dom";
import{useState,useEffect,useRef} from 'react';

const SearchBar = props => {

    const navigate=useNavigate();
    const [query, setQuery] = useState('');
    const {data:users,error:usersError,isLoading:usersIsLoading} = useFetch("http://localhost:5013/User/GetAllUsers");
    const searchContainerRef = useRef(null);
  
    useEffect(() => {
        const handleClickOutside = (event) => {
          if (searchContainerRef.current && !searchContainerRef.current.contains(event.target)) {
            let x=document.getElementById("searchDropDown")
            if(x!=null)
                x.classList.remove("show")
            setQuery('');
          }
        };
    
        document.addEventListener('click', handleClickOutside);
    
        return () => {
          document.removeEventListener('click', handleClickOutside);
        };
    }, [searchContainerRef]);

      
    const searchChange = (value) => {
        setQuery(value);
        let x=document.getElementById("searchDropDown")
        if(value.length > 0){
          x && x.classList.add("show")
        }
        else{
          x && x.classList.remove("show")
        }
    }

    return (
        <div className="search-container" >
          <input className="searchInput" ref={searchContainerRef} type="text" value={query} onChange={(e) => searchChange(e.target.value)} placeholder="Pretraži..." />
          {usersError ? (
            usersIsLoading
          ) : users ? (
            <div id="searchDropDown" className={`searchDropdown ${query.length > 0 ? 'visible' : ''}`}>
              {users
                .filter((user) => user.name.toLowerCase().includes(query.toLowerCase()) && user.name!=="random")
                .map((user, index) => {
                  return (
                    <div className="userSearch" key={index} onClick={()=>{props.onClick(user.name)}}>
                      {user.name}
                    </div>
                  );
                })}
            </div>
          ) : (
            <div></div>
          )}
        </div>
      );
}
export default SearchBar;