namespace RPG.Utils {
    public static class PropertyConstants {
        public const float CANVAS_SIZE = 4000f;
        public const float BACKGROUND_SIZE = 50f;
        public const string ITEMS_PATH = "Items";
        public const string QUESTS_PATH = "Quests";
        
        
        // SERVER

        public const string BACKEND_HOST = "localhost";
        public const string BACKEND_PORT = "8000";

        public static string SERVER_DOMAIN => $"http://{BACKEND_HOST}:{BACKEND_PORT}";
    }
}
