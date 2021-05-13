using System.Collections.Generic;


namespace Hookah_Advisor
{
    public class UserController
    {
        public interface ICondition
        {
            public List<ICommand> Commands { get; }
        }

        public class StartCondition : ICondition
        {
            public List<ICommand> Commands
            {
                get { return new List<ICommand> {new StartCommand()}; }
            }
        }

        public class DistributionWaitingCondition : ICondition
        {
            public List<ICommand> Commands
            {
                get { return new List<ICommand> { new DistributionReadingCommand() }; }
            }
        }

        public class DistributionParamsWaitingCondition : ICondition
        {
            public List<ICommand> Commands
            {
                get { return new List<ICommand> { new ParameterReadingCommand() }; }
            }
        }

        public class MethodWaitingCondition : ICondition
        {
            public List<ICommand> Commands
            {
                get { return new List<ICommand> { new MethodReadingCommand() }; }
            }
        }

        public class MethodArgsWaitingCondition : ICondition
        {
            public List<ICommand> Commands
            {
                get { return new List<ICommand> { new MethodArgsWaitingCommand(), new ChangesCommand(), }; }
            }
        }
    }
}