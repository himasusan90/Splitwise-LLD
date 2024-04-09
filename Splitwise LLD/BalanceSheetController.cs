
namespace Splitwise_LLD
{
    public class BalanceSheetController
    {
        internal void updateUserExpenseBalanceSheet(User expensePaidBy, List<Split> splits, double totalExpenseAmount)
        {
            UserExpenseBalanceSheet paidByUserExpenseSheet = expensePaidBy.getUserExpenseBalanceSheet();
            paidByUserExpenseSheet.totalPayment=paidByUserExpenseSheet.totalPayment + totalExpenseAmount;

            foreach (Split split in splits)
            {

                User userOwe = split.getUser();
                UserExpenseBalanceSheet oweUserExpenseSheet = userOwe.getUserExpenseBalanceSheet();
                double oweAmount = split.getAmountOwe();

                if (expensePaidBy.getUserId().Equals(userOwe.getUserId()))
                {
                    paidByUserExpenseSheet.totalYourExpense+= oweAmount;
                }
                else
                {

                    //update the balance of paid user
                    paidByUserExpenseSheet.totalYourExpense = paidByUserExpenseSheet.totalYouGetBack + oweAmount;

                    Balance userOweBalance;
                    if (paidByUserExpenseSheet.UserVsBalance.ContainsKey(userOwe.getUserId()))
                    {

                        userOweBalance = paidByUserExpenseSheet.UserVsBalance[userOwe.getUserId()];
                    }
                    else
                    {
                        userOweBalance = new Balance();
                        paidByUserExpenseSheet.UserVsBalance[userOwe.getUserId()]= userOweBalance;
                    }

                    userOweBalance.amountgetBack+=oweAmount;


                    //update the balance sheet of owe user
                    oweUserExpenseSheet.totalYouOwe+= oweAmount;
                    oweUserExpenseSheet.totalYourExpense+= oweAmount;

                    Balance userPaidBalance;
                    if (oweUserExpenseSheet.UserVsBalance.ContainsKey(expensePaidBy.getUserId()))
                    {
                        userPaidBalance = oweUserExpenseSheet.UserVsBalance[expensePaidBy.getUserId()];
                    }
                    else
                    {
                        userPaidBalance = new Balance();
                        oweUserExpenseSheet.UserVsBalance[expensePaidBy.getUserId()]= userPaidBalance;
                    }
                    userPaidBalance.amountgetBack += oweAmount;
                }
            }

        }

        public void showBalanceSheetOfUser(User user)
        {

            Console.WriteLine("---------------------------------------");

            Console.WriteLine("Balance sheet of user : " + user.getUserId());

            UserExpenseBalanceSheet userExpenseBalanceSheet = user.getUserExpenseBalanceSheet();

            Console.WriteLine("TotalYourExpense: " + userExpenseBalanceSheet.totalYourExpense);
            Console.WriteLine("TotalGetBack: " + userExpenseBalanceSheet.totalYouGetBack);
            Console.WriteLine("TotalYourOwe: " + userExpenseBalanceSheet.totalYouOwe);
            Console.WriteLine("TotalPaymnetMade: " + userExpenseBalanceSheet.totalPayment);
            foreach (var entry in userExpenseBalanceSheet.UserVsBalance)
            {
                string userID = entry.Key;
                Balance balance = entry.Value;

                Console.WriteLine($"userID: {userID} YouGetBack: {balance.amountgetBack} YouOwe: {balance.amountOwe}");
            }

            Console.WriteLine("---------------------------------------");

        }

    }
}