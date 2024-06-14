namespace RPG.Utils.Constants {
    public static class PropertyConstants {
        public const float CANVAS_SIZE = 4000f;
        public const float BACKGROUND_SIZE = 50f;
        public const string ITEMS_PATH = "Items";
        public const string QUESTS_PATH = "Quests";
        public const string DIALOGS_PATH = "Dialogs";
        public const int ITEM_HEIGHT = 100;
        public const int MAX_PLAYERS = 4;

        
        // SERVER

        public const string BACKEND_HOST = "localhost";
        public const string BACKEND_PORT = "8000";

        public static string SERVER_DOMAIN => $"http://{BACKEND_HOST}:{BACKEND_PORT}";
    }
}
