import axios from "axios"

export const FetchURLs = async () => {
    try{        
        var response = await axios.get("/URLs");        

        return response.data;
    } catch(e){
        console.error(e);
    }    
}

export const FetchIsAuthenticated = async () => {
    try{        
        var response = await axios.get("/isAuthenticated");  

        return response.data;
    } catch(e){
        console.error(e);
    }    
}

export const AddNewURL = async (url) => {
    try{        
        var response = await axios.post("/addURL", JSON.stringify(url), 
        {
            headers: {
              'Content-Type': 'application/json'
            }
        });  

        console.log(response)

        return response.data;
    } catch(e){
        console.error(e);
    }    
}

export const DeleteURL = async (id) => {
    try{        
        var response = await axios.post("/deleteURL", id,
        {
            headers: {
              'Content-Type': 'application/json'
            }
        });          

        console.log(response)

        return response.data;
    } catch(e){
        console.error(e);
    }    
}

export const GetUserId = async () => {
    try{        
        var response = await axios.get("/id");          

        console.log(response)

        return response.data;
    } catch(e){
        console.error(e);
    }  
}

export const IsUserAdmin = async () => {
    try{        
        var response = await axios.get("/isAdmin");          

        console.log(response)

        return response.data;
    } catch(e){
        console.error(e);
    }  
}