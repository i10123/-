package employees;

public class HR extends Employee {
    private int meetings;

    public HR(String name, int age, String workArea, int meetings) {
        super(name, age, workArea);
        this.meetings = meetings;
    }

    public void setMeetings(int meeting) { 
        this.meetings = meeting; 
    }
    public int getMeetings() { 
        return this.meetings; 
    }

    @Override
    public void work() {
        System.out.println(getName() + " проводит " + meetings + " собеседований");
    }

    @Override
    public String toString() {
        return super.toString() + "\tСобеседования: " + meetings;
    }
}