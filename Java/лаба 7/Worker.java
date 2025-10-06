public class Worker extends Employee {
    private String shift;

    public Worker(String name, int age, String department, String shift) {
        super(name, age, department);
        this.shift = shift;
    }

    public void setShift(String shift) { this.shift = shift; }
    public String getShift() { return this.shift; }

    @Override
    public void work() {
        System.out.println(getName() + " работает на смене: " + shift);
    }

    @Override
    public String toString() {
        return super.toString() + ", Смена: " + shift;
    }
}