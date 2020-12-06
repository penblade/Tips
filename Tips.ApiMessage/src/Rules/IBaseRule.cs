using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tips.Rules
{
    public interface IBaseRule<TRequest, TResponse>
    {
        RuleStatusType Status { get; }

        bool IsNotProcessed() => Status == RuleStatusType.NotProcessed;
        bool IsSkipped() => Status == RuleStatusType.Skipped;
        bool IsFailed() => Status == RuleStatusType.Failed;
        bool IsPassed() => Status == RuleStatusType.Passed;

        bool ContinueProcessing { get; }

        Task ProcessAsync(TRequest request, TResponse response, IEnumerable<IBaseRule<TRequest, TResponse>> rules);
    }
}
