import { useState, useEffect } from 'react'
import { FetchURLs, FetchIsAuthenticated, AddNewURL, DeleteURL, GetUserId, IsUserAdmin } from '../Services/URLShortenerAPI';
import {Button, 
  TextField, 
  TableContainer, 
  Table, 
  TableBody, 
  TableHead, 
  TableCell, 
  TableRow, 
  Paper,
  IconButton} from '@mui/material'
import DeleteIcon from '@mui/icons-material/Delete';
import './App.css'

export default function App() {
  const [URLs, setURLs] = useState([]);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isUserAdmin, setIsUserAdmin] = useState(false);
  const [userId, setUserId] = useState(0);
  const [newURL, setNewURL] = useState("");
  const [errMessage, setErrMessage] = useState("");

  useEffect(() => {
    const fetchData = async() => {
      let data = await FetchURLs();
      let isAuth = await FetchIsAuthenticated();
      
      if (data != null){
        setURLs(data);   
      } 

      if (isAuth != null){
        setIsAuthenticated(isAuth);

        setUserId(await GetUserId());

        setIsUserAdmin(await IsUserAdmin());
      }
    };

    fetchData();
  }, []);

  const onSubmit = async (e) =>
  {
    e.preventDefault();    
    let response = await AddNewURL(newURL);
    if (response == "Created"){
      setNewURL("");
      let data = await FetchURLs();
      setURLs(data); 
      setErrMessage("");
    }
    else{
      setErrMessage(response);
    }
  }

  const onDelete = async (id) =>
  {
    await DeleteURL(id);
    let data = await FetchURLs();
    setURLs(data); 
  }

  return (
    <>
      <div className='w-2/4'>
        <h1 className="text-3xl font-bold mb-3">URLs</h1>

        <TableContainer component={Paper} className='mb-3'>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell align="right">#</TableCell>
              <TableCell align="right">Long URL</TableCell>
              <TableCell align="right">Short URL</TableCell>              
              <TableCell align="right"></TableCell>
              <TableCell align="right"></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>        
              {URLs.map((u) => (                
              <TableRow key={u.id}>
                <TableCell align="right">{u.id}</TableCell>
                <TableCell align="right">{u.longURL}</TableCell>
                <TableCell align="right">{u.shortURL}</TableCell>
                <TableCell align="right">
                {isAuthenticated &&                  
                    <a href={`/Home/Info/${u.id}`}>Info</a>                
                }
                </TableCell>                
                {isAuthenticated && isUserAdmin || u.creatorId === userId ? (             
                <TableCell align="right">
                  <IconButton onClick={() => onDelete(u.id)}>
                    <DeleteIcon />
                  </IconButton>
                </TableCell>
                ) : null}
              </TableRow>))}
          </TableBody>
        </Table>
        </TableContainer>     
      </div>   
        
      {isAuthenticated &&
        <form onSubmit={onSubmit} className="flex flex-col w-1/4">
          <h1 className="font-bold text-3xl mb-3">Add new URL</h1>
          <div className="mb-3">
            <TextField  
              value={newURL}              
              required
              id="outlined-required"    
              onChange={(e) => setNewURL(e.target.value)} 
            />   
          </div>
          <Button variant="contained" type ="submit">Add</Button>  
          <p className='text-red-400 mt-3'>{errMessage}</p>              
        </form>}
    </>
  )
}