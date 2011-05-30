namespace Caliburn.EventAggregation {
    public class LeftEvent {
        public int Number;

        public override string ToString() {
            return "Left " + Number;
        }
    }
}