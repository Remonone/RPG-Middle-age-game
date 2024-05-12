namespace RPG.UI.Create {
    public class SessionData {
        public string Id { get; private set; }
        public string HostId { get; private set; }
        public string SessionMap { get; private set; }
        public int Level { get; private set; }

        public SessionData(string id, string hostId, string sessionMap, int level) {
            this.Id = id;
            this.HostId = hostId;
            this.SessionMap = sessionMap;
            this.Level = level;
        }
    }
}
