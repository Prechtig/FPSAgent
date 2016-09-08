package org.mma.imagerecognition.dataobjects;

public class Feature {
	private String value;
	private int type;
	
	public Feature(String value, int type) {
		super();
		this.value = value;
		this.type = type;
	}

	public String getValue() {
		return value;
	}

	public void setValue(String value) {
		this.value = value;
	}

	public int getType() {
		return type;
	}

	public void setType(int type) {
		this.type = type;
	}
	
	
}
