
namespace Splitwise_LLD
{
    public interface ExpenseSplit
    {
        internal void validateSplitRequest(List<Split> splitDetails, double expenseAmount);
       
    }
}