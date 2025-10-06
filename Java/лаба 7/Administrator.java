public class Administrator extends Employee {
    private int level;

    public Administrator(String name, int age, String department, int level) {
        super(name, age, department);
        this.level = level;
    }

    public void setLevel(int level) { this.level = level; }
    public int getLevel() { return this.level; }

    @Override
    public void work() {
        System.out.println(getName() + " управляет на уровне: " + level);
    }

    @Override
    public String toString() {
        return super.toString() + ", Уровень управления: " + level;
    }
}
