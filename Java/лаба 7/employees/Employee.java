package employees;
import java.util.Objects;

public abstract class Employee {
    private String name;                    // имя
    private int age;                        // возраст
    private String workArea;                // рабочая область
    private static int countWorkers = 0;    // счетчик работников

    public Employee(String name, int age, String workArea) {
        this.name = name;
        this.age = age;
        this.workArea = workArea;
        countWorkers++;
    }

    public void setName(String name) { 
        this.name = name; 
    }
    public void setAge(int age) { 
        this.age = age; 
    }
    public void setWorkArea(String workArea) { 
        this.workArea = workArea;
    }

    public String getName() { 
        return this.name; 
    }
    public int getAge() { 
        return this.age; 
    }
    public String getWorkArea() { 
        return this.workArea; 
    }

    public abstract void work();

    @Override
    public boolean equals(Object employeerObject) {
        if (employeerObject == null || getClass() != employeerObject.getClass()) 
            return false;

        Employee other = (Employee) employeerObject;

        return age == other.age &&
            Objects.equals(name, other.name) &&
            Objects.equals(workArea, other.workArea);
    }

    @Override
    public String toString() {
        return getClass().getSimpleName() +
               "\n\tИмя: " + name +
               "\n\tВозраст: " + age +
               "\n\tОтдел: " + workArea + "\n";
    }

    public static int getCountWorkers() {
        return countWorkers;
    }
}