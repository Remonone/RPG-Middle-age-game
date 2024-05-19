namespace RPG.Utils.Constants {
    public static class BackendCalls {
        public const string FETCH_USER = "api/v1/user/get";
        public const string SAVE_USER = "api/v1/user/save";
        public const string REGISTER_USER = "api/v1/user/register";
        public const string LOAD_USER = "api/v1/user/load";
        
        //LOBBY
        public const string LOAD_LOBBIES = "api/v1/lobby/";
        public const string FETCH_LOBBY = "api/v1/lobby/receive";
        public const string CREATE_LOBBY = "api/v1/lobby/create";
        public const string DELETE_LOBBY = "api/v1/lobby/delete";
        
        // SESSION
        public const string CREATE_WORLD = "api/v1/session/create";
        public const string FETCH_WORLDS = "api/v1/session/fetch";
        
        // CONTENT
        public const string GET_CONTENT = "api/v1/content/";
    }
}
