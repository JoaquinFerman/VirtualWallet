namespace PrimeraWebAPI.Models {
    public class LoanModel {
        public int Id { get; set; }
        private HashSet<string> validTypes = new HashSet<string> { "SimpleInterest", "CompoundInterest", "Mortgage" };
        public string LoanType { get; set; }
        private float vvalue;
        public float Value {
            get { return vvalue; }
            set { vvalue = value; RemainingValue = value; }
            }
        public float RemainingValue { get; set; }
        public float Interest { get; set; }
        public DateTime LastPaid { get; set; }
        public bool Defaulting { get; set; }

        public LoanModel(float value, string loanType, float interest = 0) : this() {
            LoanType = validTypes.Contains(loanType) ? loanType : "SimpleInterest";
            Value = value;
            RemainingValue = value;
            Interest = interest;
            LastPaid = DateTime.Now.AddMonths(1);
            Defaulting = false;
        }
        public LoanModel() {
            Id = 0;
            LoanType = "";
            Value = 0;
            RemainingValue = 0;
            Interest = 0;
            LastPaid = DateTime.Now.AddMonths(1);
            Defaulting = false;
        }

        public void Pay(float amount) {
            RemainingValue -= amount;
        }

        public void UpdateInterest(User user) {
            switch (LoanType) {
                case "SimpleInterest":
                    while(LastPaid < DateTime.Now) {
                        LastPaid = LastPaid.AddMonths(1);
                        RemainingValue += Value * (Interest / 100);
                    }
                    break;
                case "CompoundInterest":
                    while(LastPaid < DateTime.Now) {
                        LastPaid = LastPaid.AddMonths(1);
                        RemainingValue += RemainingValue * (Interest / 100);
                    }
                    break;
                case "Mortgage":
                    if(LastPaid < DateTime.Now && RemainingValue > 0) {
                        Defaulting = true;
                        LoansService.DefaultLoan(UsersService.GetUsers().Find(u => u.Username == user.Username), this);
                    }
                    break;
            }
        }
    }
}