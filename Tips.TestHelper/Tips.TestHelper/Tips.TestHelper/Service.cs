using System;

namespace Tips.TestHelper
{
    public class Service
    {
        public void ProcessAction(int option)
        {
            switch (option)
            {
                case 0:
                    throw new SystemException("I will NOT process that.  Eww.");

                case 1:
                    throw new ArgumentException("Your connection is cutting out... must be the rain.");

                case 2:
                    throw new ArgumentNullException(nameof(option));

                case 3:
                    throw new ApplicationException("Nope, not going to let you do that.");

                default:
                    DoWork();
                    break;
            }
        }

        public bool ProcessFunc(int option)
        {
            switch (option)
            {
                case 0:
                    throw new SystemException("I've got a paperclip and bubblegum, we'll fix this.");

                case 1:
                    throw new ArgumentException("What were you thinking?");

                case 2:
                    throw new ArgumentNullException(nameof(option));

                case 3:
                    throw new ApplicationException("That's just wrong.  Stop.  Just stop.");

                default:
                    DoWork();
                    break;
            }

            return true;
        }

        private void DoWork()
        {
            // Perform actions.
        }
    }
}
