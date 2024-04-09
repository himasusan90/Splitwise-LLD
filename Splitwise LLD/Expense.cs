using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Splitwise_LLD
{
    public class Expense
    {
        public string ExpenseId { get; set; }
        public string Description { get; set; }
        public double ExpenseAmount { get; set; }
        public User PaidBy { get; set; }
        public ExpenseSplitType ExpenseSplitType { get; set; }
        List<Split> SplitList { get; set; } = new List<Split>();
        public Expense(string expenseId, double expenseAmount, string description,
                   User paidByUser, ExpenseSplitType splitType, List<Split> splitDetails
)
        {
            ExpenseId = expenseId; Description = description; PaidBy = paidByUser; SplitList.AddRange(splitDetails);
            ExpenseAmount = expenseAmount; ExpenseSplitType = splitType;
        }

    }

    public class User
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public UserExpenseBalanceSheet userExpenseBalanceSheet { get; set; }

        public User(string id, string userName)
        {
            this.userId = id;
            this.userName = userName;
            userExpenseBalanceSheet = new UserExpenseBalanceSheet();
        }
        public string getUserId()
        {
            return userId;
        }
        public UserExpenseBalanceSheet getUserExpenseBalanceSheet()
        {
            return userExpenseBalanceSheet;
        }
    }

    public class UserController
    {
        public List<User> UserList { get; set; }=new List<User>() { };

        public void addUser(User user)
        {
            UserList.Add(user);
        }

        public User getUser(String userID)
        {

            foreach(var user in UserList)
            {
                if (user.getUserId().Equals(userID))
                {
                    return user;
                }
            }
            return null;
        }

        public List<User> getAllUsers()
        {
            return UserList;
        }

    }
    public class UserExpenseBalanceSheet
    {
        public Dictionary<string,Balance> UserVsBalance { get; set; }=new Dictionary<string, Balance>();
        public double totalYourExpense { get; set; }
        public double totalPayment { get; set; }
        public double totalYouOwe { get; set; }
        public double totalYouGetBack { get; set; }
    }

    public class Balance
    {
        public double amountOwe { get; set; }
        public double amountgetBack { get; set; }
    }

    public class Group
    {
        public string groupId { get; set; }
        public string groupName { get; set; }
        public List<User> groupMembers { get; set; } = new List<User>();
        public List<Expense> ExpenseList { get; set; } = new List<Expense>();
        public ExpenseController ExpenseController { get; set; }= new ExpenseController();

        public void addMember(User member)
        {
            groupMembers.Add(member);
        }

        public String getGroupId()
        {
            return groupId;
        }

        public void setGroupId(String groupId)
        {
            this.groupId = groupId;
        }

        public void setGroupName(String groupName)
        {
            this.groupName = groupName;
        }

        public Expense createExpense(String expenseId, String description, double expenseAmount,
                                     List<Split> splitDetails, ExpenseSplitType splitType, User paidByUser)
        {

            Expense expense = ExpenseController.createExpense(expenseId, description, expenseAmount, splitDetails, splitType, paidByUser);
            ExpenseList.Add(expense);
            return expense;
        }


    }

    public class GroupController
    {
        public List<Group> GroupList { get; set; } = new List<Group>();

        public void AddNewGroup(string groupId, string groupName, User createdByUser)
        {
            Group group = new Group();
            group.groupId= groupId;
            group.groupName= groupName;
            group.addMember(createdByUser);
           GroupList.Add(group);

        }
        public Group getGroup(String groupId)
        {

            foreach (Group group in GroupList)
            {

                if (group.getGroupId().Equals(groupId))
                {
                    return group;
                }
            }
            return null;
        }


    }

    public class Split
    {
        User user;
        double amountOwe;

        public Split(User user, double amountOwe)
        {
            this.user = user;
            this.amountOwe = amountOwe;
        }

        public User getUser()
        {
            return user;
        }

        public void setUser(User user)
        {
            this.user = user;
        }

        public double getAmountOwe()
        {
            return amountOwe;
        }

        public void setAmountOwe(double amountOwe)
        {
            this.amountOwe = amountOwe;
        }


    }
    public enum ExpenseSplitType
    {
        Equal,
        NotEqual,
        Percentage
    }
    public class ExpenseController
    {

        BalanceSheetController balanceSheetController;
        public ExpenseController()
        {
            balanceSheetController = new BalanceSheetController();
        }



        public Expense createExpense(String expenseId, String description, double expenseAmount,
                                     List<Split> splitDetails, ExpenseSplitType splitType, User paidByUser)
        {

            ExpenseSplit expenseSplit = SplitFactory.getSplitObject(splitType);
            expenseSplit.validateSplitRequest(splitDetails, expenseAmount);

            Expense expense = new Expense(expenseId, expenseAmount, description, paidByUser, splitType, splitDetails);

            balanceSheetController.updateUserExpenseBalanceSheet(paidByUser, splitDetails, expenseAmount);

            return expense;
        }

    }

    internal class SplitFactory
    {
        internal static ExpenseSplit getSplitObject(ExpenseSplitType splitType)
        {
            switch (splitType)
            {
                case ExpenseSplitType.Equal:
                    return new EqualExpenseSplit();
                case ExpenseSplitType.NotEqual:
                    return new NotEqualExpenseSplit();
                case ExpenseSplitType.Percentage:
                    return new PercentageExpenseSplit();
                default:
                    return null;
            }

        }
    }

    internal class PercentageExpenseSplit : ExpenseSplit
    {
        void ExpenseSplit.validateSplitRequest(List<Split> splitDetails, double expenseAmount)
        {
            throw new NotImplementedException();
        }
    }

    internal class NotEqualExpenseSplit : ExpenseSplit
    {
        void ExpenseSplit.validateSplitRequest(List<Split> splitDetails, double expenseAmount)
        {
            throw new NotImplementedException();
        }
    }

    internal class EqualExpenseSplit : ExpenseSplit
    {
        void ExpenseSplit.validateSplitRequest(List<Split> splitDetails, double expenseAmount)
        {
            double amountShouldBePresent = expenseAmount / splitDetails.Count;
            foreach (Split split in splitDetails) 
            {
                if (split.getAmountOwe() != amountShouldBePresent)
                {
                    //throw exception
                }
            }

        }
    }

    public class SplitWise
    {
        public UserController userController { get; set; }
        public GroupController groupController { get; set; }
        public BalanceSheetController balanceSheetController { get; set; }
        public SplitWise()
        {
                userController = new UserController();
            groupController = new GroupController();
            balanceSheetController = new BalanceSheetController();
        }
        public void demo()
        {

            setupUserAndGroup();

            //Step1: add members to the group
            Group group = groupController.getGroup("G1001");
            group.addMember(userController.getUser("U2001"));
            group.addMember(userController.getUser("U3001"));

            //Step2. create an expense inside a group
            List<Split> splits = new List<Split>();
            Split split1 = new Split(userController.getUser("U1001"), 300);
            Split split2 = new Split(userController.getUser("U2001"), 300);
            Split split3 = new Split(userController.getUser("U3001"), 300);
            splits.Add(split1);
            splits.Add(split2);
            splits.Add(split3);
            group.createExpense("Exp1001", "Breakfast", 900, splits, ExpenseSplitType.Equal, userController.getUser("U1001"));

            List<Split> splits2 = new List<Split>();
            Split splits2_1 = new Split(userController.getUser("U1001"), 400);
            Split splits2_2 = new Split(userController.getUser("U2001"), 100);
            splits2.Add(splits2_1);
            splits2.Add(splits2_2);
            group.createExpense("Exp1002", "Lunch", 500, splits2, ExpenseSplitType.NotEqual, userController.getUser("U2001"));

            foreach (User user in userController.getAllUsers())
            {
                balanceSheetController.showBalanceSheetOfUser(user);
            }
        }

        public void setupUserAndGroup()
        {

            //onboard user to splitwise app
            addUsersToSplitwiseApp();

            //create a group by user1
            User user1 = userController.getUser("U1001");
            groupController.AddNewGroup("G1001", "Outing with Friends", user1);
        }

        private void addUsersToSplitwiseApp()
        {

            //adding User1
            User user1 = new User("U1001", "User1");

            //adding User2
            User user2 = new User("U2001", "User2");

            //adding User3
            User user3 = new User("U3001", "User3");

            userController.addUser(user1);
            userController.addUser(user2);
            userController.addUser(user3);
        }

    }
}