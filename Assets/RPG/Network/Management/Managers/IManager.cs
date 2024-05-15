namespace RPG.Network.Management.Managers {
    public interface IManager {
        
        public string Token { get; set; }

        public void Disconnect();
    }
}
