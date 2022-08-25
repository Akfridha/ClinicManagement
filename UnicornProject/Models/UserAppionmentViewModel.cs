namespace UnicornProject.Models
{
    public class UserAppionmentViewModel
    {
        public int Id { get; set; }
        public UserModel User { get; set; }

        public UserModel Doctor { get; set; }

        public AppionmentModel Appionment { get; set; }

    }
}

