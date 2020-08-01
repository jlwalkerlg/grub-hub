import React, { useState, SyntheticEvent, useRef } from "react";
import { Rule, RequiredRule } from "./Rule";

export interface FormComponent {
  value: string;
  touched: boolean;
  dirty: boolean;
  valid: boolean;
  error: string;
  validate(): boolean;
  props: {
    value: string;
    valid: "true" | "false" | undefined;
    touched: "true" | "false" | undefined;
    dirty: "true" | "false" | undefined;
    onChange(e: SyntheticEvent<HTMLInputElement>): void;
    onBlur(e: SyntheticEvent): void;
  };
}

export interface UseFormComponentOptions {}

export function useFormComponent(
  initialValue: string,
  rules: Rule[] = [],
  options: UseFormComponentOptions = {}
): FormComponent {
  const required = useRef(
    rules.filter((x) => x instanceof RequiredRule).length > 0
  ).current;

  const [value, setValue] = useState(initialValue);
  const [touched, setTouched] = useState(false);
  const [dirty, setDirty] = useState(false);
  const [error, setError] = useState<string>(null);
  const [valid, setValid] = useState(!required);

  function validate(v: string = value): boolean {
    for (const rule of rules) {
      const error = rule.validate(v);

      if (error !== null) {
        setError(error);
        setValid(false);
        return false;
      }
    }

    setError(null);
    setValid(true);
    return true;
  }

  function onBlur(e: SyntheticEvent<HTMLInputElement>): void {
    setTouched(true);
    validate(e.currentTarget.value);
  }

  function onChange(e: SyntheticEvent<HTMLInputElement>): void {
    setDirty(true);
    setValue(e.currentTarget.value);
    validate(e.currentTarget.value);
  }

  return {
    value,
    touched,
    dirty,
    valid,
    error,
    validate,
    props: {
      value,
      valid: valid ? "true" : "false",
      touched: touched ? "true" : "false",
      dirty: dirty ? "true" : "false",
      onChange,
      onBlur,
    },
  };
}
