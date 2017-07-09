namespace Hazelnut.Core.HUsers {
    public class HUser {

        public HUser(int huserId, string huserName) {
            HUserId = huserId;
            HUserName = huserName;
        }

        public int HUserId { get; set; }
        public string HUserName { get; set; }
    }
}