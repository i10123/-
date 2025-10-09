package employees;

public class Worker extends Employee {
    private String workPeriod;

    public Worker(String name, int age, String workArea, String workPeriod) {
        super(name, age, workArea);
        this.workPeriod = workPeriod;
    }

    public void setWorkPeriod(String workPeriod) { 
        this.workPeriod = workPeriod; 
    }
    public String getWorkPeriod() { 
        return this.workPeriod; 
    }

    @Override
    public void work() {
        String formattedPeriod = switch (workPeriod.toLowerCase()) {
            case "дневная" -> "в дневную смену";
            case "ночная" -> "в ночную смену";
            case "утреняя" -> "в утренюю смену";
            case "вечерняя" -> "в вечернюю смену";
            default -> "в смену: " + workPeriod;
        };
        System.out.println(getName() + " работает " + formattedPeriod);
    }

    @Override
    public String toString() {
        return super.toString() + "\tСмена: " + workPeriod;
    }
}