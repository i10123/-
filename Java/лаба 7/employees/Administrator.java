package employees;

public class Administrator extends Employee {
    private int level;

    public Administrator(String name, int age, String workArea, int level) {
        super(name, age, workArea);
        this.level = level;
    }

    public void setLevel(int level) { 
        this.level = level;
    }
    public int getLevel() { 
        return this.level; 
    }

    @Override
    public void work() {
        System.out.println(getName() + " управляет на уровне: " + level);
    }

    @Override
    public String toString() {
        return super.toString() + "\tУровень управления: " + level;
    }
}