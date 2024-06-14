namespace RPG.UI.Create {
    public class SessionData {
        public string Id { get; init; }
        public string HostId { get; init; }
        public string SessionMap { get; init; }
        public int Level { get; init; }

        public string SessionName { get; private set; }

        public SessionData(string id, string hostId, string sessionMap, int level, string name) {
            this.Id = id;
            this.HostId = hostId;
            this.SessionMap = sessionMap;
            this.Level = level;
            this.SessionName = name;
        }
    }
}
