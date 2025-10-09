package employees;

public class Engineer extends Employee {
    private String specialization;

    public Engineer(String name, int age, String workArea, String specialization) {
        super(name, age, workArea);
        this.specialization = specialization;
    }

    public void setSpecialization(String specialization) { 
        this.specialization = specialization; 
    }
    public String getSpecialization() { 
        return this.specialization; 
    }

    @Override
    public void work() {
        System.out.println(getName() + " проектирует в области " + specialization);
    }

    @Override
    public String toString() {
        return super.toString() + "\tСпециализация: " + specialization;
    }
}